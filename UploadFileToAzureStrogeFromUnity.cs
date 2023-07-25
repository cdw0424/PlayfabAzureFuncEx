using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.DataModels;
using UnityEngine;
using DoodleHero;
using System;
using UnityEngine.Networking;
using System.IO;
using System.Web;
using System.Linq;

public class Test
{
    /// <summary>
    /// The image upload function to the Azure server(Blob Storage)
    /// Modify 2023-07-25 by Dongwon Choi
    /// </summary>
    /// <param name="sasToken">Azure blob Access Key for Users</param>
    /// <param name="doodleItem">To Check for ItemInfo</param>
    /// <param name="imageFilePath">The image file path for uploading to the Azure server</param>
    /// <returns></returns>
    public async Task UploadImageAsync(string sasToken, string imageFilePath)
    {

        // 이미지를 업로드할 컨테이너의 URL주소
        string uploadURL = "컨테이너의 주소";


        // 서버에 업로드될 이미지의 경로입니다.
        // 이 예제에서는 임의의 이미지를 사용합니다.
        string imagePath = $"파일명.png";


        // 업로드할 이미지의 URI입니다.
        // URI란?
        // Uniform Resource Identifier의 약자로, 인터넷에 있는 자원을 나타내는 유일한 주소입니다.
        // 더 쉽게 말하면, 인터넷에 있는 파일의 주소를 의미합니다.
        string blobUri = $"{uploadURL}/{imagePath}?{sasToken}";

        try
        {
            // 이미지를 바이트 배열로 변환합니다.
            //byte[] imageBytes = doodleItem.Image.texture.EncodeToPNG();

            byte[] imageBytes = System.IO.File.ReadAllBytes(imageFilePath);

            // C#의HTTP가아닌 유니티의 UnityWebRequest를 사용하는 이유는
            // Content-Length 등의 헤더를 자동으로 설정하기 위함입니다.
            // 수동으로 HTTP로 써도 아무런 문제가 안됩니다.
            // Content-Length는 업로드할 파일의 크기(Byte)를 나타내는 헤더입니다.
            // Push가 아닌 Put을 써야합니다.
            UnityWebRequest www = new UnityWebRequest(blobUri, "PUT");
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(imageBytes);

            // Azure의 BlobStorage에 파일을 업로드하기 위해 필요한 헤더를 추가합니다.
            // "x-ms-blob-type", "BlockBlob"이라고 입력해주세요.
            www.SetRequestHeader("x-ms-blob-type", "BlockBlob");

            // 요청 작업 상태를 확인 하기 위한 operation.
            var operation = www.SendWebRequest();

            // 요청 작업이 완료될 때까지 대기합니다.
            while (!operation.isDone)
            {
                await Task.Delay(100); // wait for completion
            }

            // 업로드 실패 시 에러를 출력합니다.
            if (www.result != UnityWebRequest.Result.Success)
            {
                TestDebug.Log("이미지 업로드 실패 : " + www.error);
            }


        }
        catch (PlayFabException e)
        {
            //TestDebug.Log(e.Message);
            //TestDebug.Log("이미지 업로드 실패");
        }
        finally
        {
            //TestDebug.Log("이미지 업로드 과정 종료");
        }
    }
}
