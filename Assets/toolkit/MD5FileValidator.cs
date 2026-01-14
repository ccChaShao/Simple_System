using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class MD5FileValidator
{
    public static string ComputeFileMD5(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"[ComputeFileMD5]：文件不存在！{filePath}");
            return string.Empty;
        }

        try
        {
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hashBytes = md5.ComputeHash(fileStream);
                    return BytesToHexString(hashBytes);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ComputeFileMD5]：计算文件MD5时出错！{ex.Message}");
            return string.Empty;
        }
    }

    public static bool VerifyFileMD5(string filePath, string expectedMD5)
    {
        if (string.IsNullOrEmpty(expectedMD5))
            return false;

        string actualMD5 = ComputeFileMD5(filePath);
        bool isValid = string.Equals(actualMD5, expectedMD5, System.StringComparison.OrdinalIgnoreCase);
        
        if (!isValid)
        {
            Debug.LogWarning($"文件MD5不匹配: 期望={expectedMD5}, 实际={actualMD5}");
        }
        
        return isValid;
    }
    
    private static string BytesToHexString(byte[] bytes)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("x2")); // "x2"表示小写十六进制格式
        }
        return sb.ToString();
    }

}