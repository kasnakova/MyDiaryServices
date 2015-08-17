namespace MyDiary.Services.Infrastructure
{
    interface ISecurityProvider
    {
        string HashPassword(string password);

        string EncryptText(string text, string key);

        string DecryptText(string text, string key);
    }
}
