using Microsoft.EntityFrameworkCore;
using MyWallet.Models;

namespace MyWallet.Data
{
    public class MyWalletContext : DbContext
    {
        public MyWalletContext(DbContextOptions<MyWalletContext> options)
            : base(options)
        {
        }

        public DbSet<MonthlyBudget> MonthlyBudgets { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<SubjectName> SubjectNames { get; set; }

        // 科目ImageDB登録用メソッド
        public void SeedImagesFromWwwroot() // wwwroot/Images フォルダ内のPNGファイルを SubjectName テーブルに登録するメソッド
        {
            try
            {
                // wwwroot/Images の絶対パスを取得
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
                if (!Directory.Exists(imagesPath))
                {
                    Console.WriteLine($"[ERROR] 画像フォルダが存在しません: {imagesPath}");
                    return;
                }
                // PNGファイル一覧を取得
                var pngFiles = Directory.GetFiles(imagesPath, "*.png", SearchOption.TopDirectoryOnly);
                foreach (var filePath in pngFiles)
                {
                    var fileName = Path.GetFileName(filePath);
                    // DBに保存する相対パス（例: "images/sample.png"）
                    var relativePath = Path.Combine("Images", fileName).Replace("\\", "/");
                    // 重複チェック
                    if (!SubjectNames.Any(s => s.ImageUrl == relativePath))
                    {
                        var subject = new SubjectName
                        {
                            //SubjectNameIdは自動生成されるため、指定しない
                            CourseName = Path.GetFileNameWithoutExtension(fileName), // ファイル名をName列に設定（拡張子なし）
                            ImageUrl = relativePath
                        };
                        SubjectNames.Add(subject);  // DBに追加
                    }
                }
                SaveChanges();                    // 変更を保存
                Console.WriteLine("[INFO] 画像登録が完了しました。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] 登録処理中にエラー: {ex.Message}");
            }
        }

        public void SeedData(MyWalletContext db)  // データベースに初期データを挿入するメソッド
        {
            if (db.PaymentTypes.Any()) return; // データが既に存在する場合はスキップ

            // 起動時に挿入する初期データ
            var paymentTypes = new List<PaymentType>
              {
                  //PaymentTypeIdは自動生成されるため、指定しない
                  new PaymentType {TypeName = "現金" },
                  new PaymentType {TypeName = "モバイル決済" },
                  new PaymentType {TypeName = "クレジットカード" },
                  new PaymentType {TypeName = "電子マネー" },
                  new PaymentType{TypeName = "銀行自動引き落とし"}
              };
                    db.PaymentTypes.AddRange(paymentTypes);// データベースに保存
                    db.SaveChanges();                    // 変更を保存
        }
    }
}
