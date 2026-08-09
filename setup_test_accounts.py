# -*- coding: utf-8 -*-
import hashlib
import json
import ssl
import urllib.request
import time
import random

SERVER_URL = "https://bzhero.online:8443"

ssl_ctx = ssl.create_default_context()
ssl_ctx.check_hostname = False
ssl_ctx.verify_mode = ssl.CERT_NONE

def md5_with_salt(s):
    data = (s + "KJL").encode("utf-8")
    return hashlib.md5(data).hexdigest()

def post_json(url, body_dict):
    body = json.dumps(body_dict).encode("utf-8")
    req = urllib.request.Request(url, data=body, method="POST")
    req.add_header("Content-Type", "application/json")
    try:
        resp = urllib.request.urlopen(req, context=ssl_ctx, timeout=10)
        return json.loads(resp.read().decode("utf-8"))
    except Exception as e:
        return {"code": -1, "error": str(e)}

def make_unlocked_heroes(pass_level):
    count = pass_level // 10
    if count <= 0:
        return ""
    return ",".join(str(i) for i in range(1, count + 1))

password_md5 = md5_with_salt("123456")
heads = ["tile_1","tile_2","tile_3","tile_4","tile_5","tile_6","tile_7","tile_8"]

# 所有需要设置数据的账号（跳过注册，直接登录）
accounts = [
    # 1-3: 空白账号，不需要设置
    {"account": "1001", "nick": None, "pass_level": 0, "items": 0, "diamond": 0},
    {"account": "1002", "nick": None, "pass_level": 0, "items": 0, "diamond": 0},
    {"account": "1003", "nick": None, "pass_level": 0, "items": 0, "diamond": 0},
    
    {"account": "2001", "nick": "勇者起步一", "pass_level": 5, "items": 10, "diamond": 0},
    {"account": "2002", "nick": "勇者起步二", "pass_level": 5, "items": 10, "diamond": 0},
    {"account": "2003", "nick": "勇者起步三", "pass_level": 5, "items": 10, "diamond": 0},
    {"account": "2011", "nick": "勇者进阶一", "pass_level": 150, "items": 100, "diamond": 500},
    {"account": "2012", "nick": "勇者进阶二", "pass_level": 150, "items": 100, "diamond": 500},
    {"account": "2013", "nick": "勇者进阶三", "pass_level": 150, "items": 100, "diamond": 500},
    {"account": "2021", "nick": "勇者大师一", "pass_level": 300, "items": 200, "diamond": 1000},
    {"account": "2022", "nick": "勇者大师二", "pass_level": 300, "items": 200, "diamond": 1000},
    {"account": "2023", "nick": "勇者大师三", "pass_level": 300, "items": 200, "diamond": 1000},
    
    {"account": "3001", "nick": "小勇者一号", "pass_level": 5, "items": 10, "diamond": 0},
    {"account": "3002", "nick": "小勇者二号", "pass_level": 5, "items": 10, "diamond": 0},
    {"account": "3011", "nick": "小勇者三号", "pass_level": 150, "items": 100, "diamond": 0},
    {"account": "3012", "nick": "小勇者四号", "pass_level": 150, "items": 100, "diamond": 0},
    {"account": "3021", "nick": "小勇者五号", "pass_level": 300, "items": 150, "diamond": 0},
    {"account": "3022", "nick": "小勇者六号", "pass_level": 300, "items": 150, "diamond": 0},
    
    {"account": "4001", "nick": "少年勇者一", "pass_level": 5, "items": 10, "diamond": 0},
    {"account": "4002", "nick": "少年勇者二", "pass_level": 5, "items": 10, "diamond": 0},
    {"account": "4011", "nick": "少年勇者三", "pass_level": 150, "items": 100, "diamond": 200},
    {"account": "4012", "nick": "少年勇者四", "pass_level": 150, "items": 100, "diamond": 200},
    {"account": "4021", "nick": "少年勇者五", "pass_level": 300, "items": 200, "diamond": 500},
    {"account": "4022", "nick": "少年勇者六", "pass_level": 300, "items": 200, "diamond": 500},
    
    {"account": "5001", "nick": "青年勇者一", "pass_level": 5, "items": 10, "diamond": 0},
    {"account": "5002", "nick": "青年勇者二", "pass_level": 5, "items": 10, "diamond": 0},
    {"account": "5011", "nick": "青年勇者三", "pass_level": 150, "items": 100, "diamond": 200},
    {"account": "5012", "nick": "青年勇者四", "pass_level": 150, "items": 100, "diamond": 200},
    {"account": "5021", "nick": "青年勇者五", "pass_level": 300, "items": 200, "diamond": 500},
    {"account": "5022", "nick": "青年勇者六", "pass_level": 300, "items": 200, "diamond": 500},
]

print("="*60)
print("  Setup: login + create role + upload data")
print(f"  Server: {SERVER_URL}")
print("="*60)

success_count = 0
fail_count = 0

for i, acc in enumerate(accounts, 1):
    account = acc["account"]
    print(f"\n[{i:2d}/30] {account}: login...", end="")
    
    result = post_json(f"{SERVER_URL}/user/login", {"Account": account, "Password": password_md5})
    if result.get("code") != 0:
        print(f" FAILED (code: {result.get('code')})")
        fail_count += 1
        continue
    
    data = result.get("data", {})
    user_id = data.get("UserId")
    token = data.get("Token")
    existing_nick = data.get("NickName", "")
    # 服务器返回的空字符串可能带引号
    if existing_nick in ('', '""', '\"\"'):
        existing_nick = ""
    print(f" ok (UserId:{user_id})")
    time.sleep(0.2)
    
    # Create role if needed
    if acc["nick"] is not None and (not existing_nick or len(existing_nick) < 5):
        head = heads[random.randint(0, 7)]
        print(f"  Create role: {acc['nick']}...", end="")
        result = post_json(f"{SERVER_URL}/user/create_role", {
            "UserId": int(user_id) if isinstance(user_id, str) else user_id,
            "Token": token,
            "HeadIcon": head,
            "NickName": acc["nick"]
        })
        if result.get("code") == 0:
            print(" ok")
        else:
            print(f" FAILED (code: {result.get('code')})")
            fail_count += 1
            continue
        time.sleep(0.2)
    elif acc["nick"] is None:
        print(f"  Blank account, skip")
    else:
        print(f"  Role exists: {existing_nick}")
    
    # Upload data if needed
    if acc["pass_level"] > 0:
        heroes = make_unlocked_heroes(acc["pass_level"])
        hero_count = acc["pass_level"] // 10
        uid = int(user_id) if isinstance(user_id, str) else user_id
        print(f"  Upload: lvl={acc['pass_level']}, items={acc['items']}, diamond={acc['diamond']}, heroes={hero_count}...", end="")
        result = post_json(f"{SERVER_URL}/game/upload_pass_level", {
            "UserId": uid,
            "PassLevel": acc["pass_level"],
            "IsFinishTutorial": 1,
            "ItemNum_Undo": acc["items"],
            "ItemNum_Shuffle": acc["items"],
            "ItemNum_Hint": acc["items"],
            "ItemNum_ExtraSlot": acc["items"],
            "ItemNum_AddTime": acc["items"],
            "DiamondCount": acc["diamond"],
            "MonthlyRechargeMonth": "",
            "MonthlyRechargeAmount": 0,
            "UnlockedHeroes": heroes
        })
        if result.get("code") == 0:
            print(" ok")
        else:
            print(f" FAILED (code: {result.get('code')})")
            fail_count += 1
            continue
        time.sleep(0.2)
    
    success_count += 1

print(f"\n{'='*60}")
print(f"  Done! Success: {success_count}, Failed: {fail_count}")
print(f"{'='*60}")
