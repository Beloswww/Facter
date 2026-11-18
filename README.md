# Facter — 人臉比對點名系統

Facter 是一套以**人臉辨識**進行課堂點名的 Web 應用。老師與學生各自註冊（含人臉照片），
上課時老師上傳課堂照片，系統透過人臉比對 API 將照片與已註冊的學生人臉逐一比對，
自動完成點名並產生出缺勤紀錄。本專案為競賽專題作品（FACTER）。

## 功能

- **老師 / 學生註冊與登入**：各自建立帳號，註冊時上傳人臉照片建檔。
- **人臉點名（Roll Call）**：老師上傳課堂照片，系統與已註冊的學生人臉比對，相似度達門檻即判定出席。
- **出缺勤紀錄**：產生每次點名的出席 / 缺席名單，老師與學生皆可查詢。

## 技術棧

- ASP.NET MVC 5（C#，.NET Framework 4.8）
- Entity Framework 6（Database First，edmx）＋ SQL Server LocalDB
- [Luxand Cloud](https://luxand.cloud/) 人臉比對 API（`/photo/similarity`）
- Razor Views、HTML / CSS、Newtonsoft.Json

## 系統架構

```
prjFaceV2/
├── prjFaceV.sln
└── prjFaceV/
    ├── Controllers/        # Home（註冊/登入）、Teacher（點名/紀錄）、Student（紀錄）
    ├── Models/             # EF 資料模型（DBMember）、tMember、sMember、點名與統計表
    ├── Views/              # Razor 畫面（登入、註冊、點名、出缺勤）
    ├── App_Data/           # 資料庫（未包含於版本庫，需自行建立）
    ├── MemberFaceImages/   # 已註冊人臉（未包含，需自行放置）
    └── UpLoadFaceImages/   # 點名時上傳的照片（未包含）
```

點名比對流程（`TeacherController`）：上傳照片 → 讀取 `MemberFaceImages` 內每張已註冊人臉 →
呼叫 Luxand `photo/similarity` 比對 → 相似度超過門檻（score > 0.90）即判定為同一人、完成點名。

## 設定與執行

> ⚠️ 基於隱私，本版本**不含**人臉照片與資料庫；API 金鑰也已移除，需自行填入。

1. **還原套件**：用 Visual Studio 開啟 `prjFaceV.sln`，建置時會自動還原 NuGet 套件。

2. **填入 Luxand API 金鑰**：到 [luxand.cloud](https://luxand.cloud/) 申請一把金鑰，
   打開 `prjFaceV/Controllers/TeacherController.cs`，把 `YOUR_LUXAND_API_KEY` 換成你的金鑰。
   （建議改放到 `Web.config` 的 appSettings，避免寫死在程式碼。）

3. **建立資料庫**：本專案為 Database First。請在 LocalDB（`(LocalDB)\MSSQLLocalDB`）建立名為
   `DBMember` 的資料庫，並依 `Models/DBMemberModel.edmx` 建立對應資料表（老師、學生、點名、統計等），
   放到 `prjFaceV/App_Data/DBMember.mdf`。

4. **準備人臉資料夾**：確認 `prjFaceV/MemberFaceImages/` 與 `prjFaceV/UpLoadFaceImages/` 存在，
   放入你自己的測試照片（請使用你有權使用、或已取得同意的照片）。

5. **執行**：在 Visual Studio 按 F5 啟動（IIS Express）。

## 隱私說明

這是人臉辨識系統，會處理人臉這類生物特徵資料。此版本已刻意排除所有真實人臉照片與資料庫，
請勿將他人的人臉照片上傳到公開倉庫，實際使用時也應取得當事人同意。
