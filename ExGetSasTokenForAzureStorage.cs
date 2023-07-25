
// 자신(관리자)의 Azure계정 환경변수에서 AzureWebJobsStorage에 담긴 계정키등의 정보를 받아옴.
private static readonly string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
  
        [FunctionName("GenerateSasToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {


            try
            {

                // 토큰생성 시작 이라고 로그 출력
                log.LogInformation("----토큰생성 시작--------------");

                CloudStorageAccount storageAccount;

                string accountName= "";
                string accountKey = "";

                // BlobServiceClient 객체를 생성합니다.
                // "AzureWebJobsStorage" 환경 변수에 Azure Storage 연결 문자열을 설정해야 합니다.
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);


                if (blobServiceClient == null)
                {
                    log.LogInformation("---------------blobServiceClient is null-------------");

                    log.LogInformation("-------connectionString : " + connectionString);
                }

                if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
                {
                     accountName = storageAccount.Credentials.AccountName;
                     accountKey = storageAccount.Credentials.ExportBase64EncodedKey();
                }
                log.LogInformation("---------------blobServiceClient is not null-------------");


                // BlobSasBuilder 객체를 생성하고 설정합니다.
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = "만들어놓은 컨테이너의 이름 입력(영어만, 대문자X)",
                    Resource = "c",  // c는 컨테이너를 의미합니다. 특정 Blob(파일)만 접근하게 하려면 b라고 쓰면 됨.
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),  // SAS 토큰의 만료 시간을 설정합니다.
                };


                // 읽기, 생성 권한을 부여합니다.
                sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Create);

                // 토큰생성 직전  
                log.LogInformation("----토큰생성 직전--------------");

                // SAS 토큰을 생성합니다.
                StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);
                string sasToken = sasBuilder.ToSasQueryParameters(sharedKeyCredential).ToString();

                // 토큰생성 완료
                log.LogInformation("----토큰생성 완료--------------");

                // 생성된 SAS 토큰을 반환합니다.
                return new OkObjectResult(sasToken);
            }
            catch (Exception e)
            {
                log.LogInformation("Exception : " + e.Message);
                return new OkObjectResult("Exception : " + e.Message);
            }


        }
