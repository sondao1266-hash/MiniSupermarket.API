import urllib.request
import json
import ssl

ctx = ssl.create_default_context()
ctx.check_hostname = False
ctx.verify_mode = ssl.CERT_NONE

BASE_URL = "http://localhost:5010"

def request(method, path, body=None, token=None):
    url = f"{BASE_URL}/{path}"
    headers = {"Content-Type": "application/json"}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    data = json.dumps(body).encode("utf-8") if body else None
    req = urllib.request.Request(url, data=data, headers=headers, method=method)
    try:
        with urllib.request.urlopen(req, context=ctx, timeout=10) as resp:
            text = resp.read().decode("utf-8")
            return resp.status, json.loads(text) if text else {}
    except urllib.error.HTTPError as e:
        text = e.read().decode("utf-8")
        try:
            return e.code, json.loads(text)
        except:
            return e.code, text
    except Exception as e:
        return 500, str(e)

results = []

def run_test(name, method, path, body=None, token=None):
    code, res = request(method, path, body, token)
    passed = 200 <= code < 300
    res_str = json.dumps(res, ensure_ascii=False) if isinstance(res, (dict, list)) else str(res)
    results.append({
        "name": name,
        "status": "PASS" if passed else "FAIL",
        "code": code,
        "detail": res_str
    })
    return res

print("=== STARTING FULL TEST SUITE ===")

# 1. Bao cao (Xem bao cao - fix loi 404)
run_test("1.1 Daily Report 2026-10-23 (Hom nay)", "GET", "api/reports/daily?date=2026-10-23")
run_test("1.2 Daily Report 2026-10-07 (Co don hang)", "GET", "api/reports/daily?date=2026-10-07")
run_test("1.3 SP Revenue By Cashier (sp_GetRevenueByCashier)", "GET", "api/reports/revenue-by-cashier")
run_test("1.4 SP Revenue By Date Range (sp_GetRevenueByDateRange)", "GET", "api/reports/revenue-by-date?from=2026-10-01&to=2026-10-31")
run_test("1.5 SP Top 5 Selling Products (sp_GetTop5SellingProducts)", "GET", "api/reports/top5-products")

# 2. Xac thuc JWT & Phan quyen
admin_login = run_test("2.1 Dang nhap Admin (admin01)", "POST", "api/auth/login", {"username": "admin01", "password": "123456"})
admin_token = admin_login.get("token") if isinstance(admin_login, dict) else None

cashier_login = run_test("2.2 Dang nhap Thu ngan (cashier01)", "POST", "api/auth/login", {"username": "cashier01", "password": "123456"})
warehouse_login = run_test("2.3 Dang nhap Kho (ware01)", "POST", "api/auth/login", {"username": "ware01", "password": "123456"})

run_test("2.4 Danh sach Users (bao ve boi JWT)", "GET", "api/users", token=admin_token)

# 3. Danh muc Kitchen Express
run_test("3.1 Lay toan bo danh muc (Categories)", "GET", "api/categories")
run_test("3.2 Tim kiem danh muc", "GET", "api/categories/search?keyword=Dao")

# 4. Hang hoa & Phan trang LINQ
run_test("4.1 Phan trang hang hoa (Page 1, Size 5)", "GET", "api/products/paged?page=1&pageSize=5")
run_test("4.2 Phan trang hang hoa loc theo CategoryId=1", "GET", "api/products/paged?page=1&pageSize=5&categoryId=1")
run_test("4.3 Danh sach hang hoa kem Category (with-category)", "GET", "api/products/with-category")
run_test("4.4 Quet ma vach Barcode POS (893000000001)", "GET", "api/products/barcode/893000000001")

# 5. Khach hang & Tich diem
run_test("5.1 Phan trang danh sach khach hang", "GET", "api/customers/paged?page=1&pageSize=5")
run_test("5.2 Tim khach hang bang SDT POS (0901)", "GET", "api/customers/search?keyword=0901")

# 6. Ban hang POS & Giao dich SqlTransaction ACID
pos_payload = {
    "cashierUsername": "cashier01",
    "customerId": 1,
    "cashReceived": 5000000,
    "items": [{"productId": 1, "quantity": 1}]
}
run_test("6.1 Thanh toan POS (OrdersController)", "POST", "api/orders/checkout", pos_payload)

ado_payload = {
    "cashierUsername": "cashier01",
    "customerId": 2,
    "cashReceived": 1000000,
    "items": [{"productId": 3, "quantity": 1}]
}
run_test("6.2 Thanh toan POS Transaction ACID (CheckoutController)", "POST", "api/checkout", ado_payload)

print("\n=== TEST RESULTS SUMMARY ===")
pass_count = 0
for r in results:
    if r['status'] == 'PASS':
        pass_count += 1
    safe_detail = r['detail'][:110].encode('ascii', errors='replace').decode('ascii')
    print(f"[{r['status']}] (HTTP {r['code']}) {r['name']}")
    print(f"       Detail: {safe_detail}")

print(f"\nTOTAL: {pass_count}/{len(results)} TESTS PASSED")

with open("e:/LTUD/MiniSupermarket-main/test_results.json", "w", encoding="utf-8") as f:
    json.dump(results, f, ensure_ascii=False, indent=2)
