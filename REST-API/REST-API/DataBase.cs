using System.Security.Cryptography;
using System.Text;

namespace REST_API
{
    public class DataBase //Please Implement you own Database here i will use a Simple Locale File Based API
    {
        #region GeneralStuff
#pragma warning disable CS8618 // I dont like having Warnings in my Console
        public static string RootLocation { get; set; }
        public static string FileLocation { get; set; }
        public static string KeyLocation { get; set; }
        public static string UserLocation { get; set; }
#pragma warning restore CS8618
        public static void Load()
        {
            RootLocation = Directory.GetCurrentDirectory() + "/REST-API";
            if (!Directory.Exists(RootLocation))
            {
                Directory.CreateDirectory(RootLocation);
            }
            FileLocation = RootLocation + "/Files";
            if (!Directory.Exists(FileLocation))
            {
                Directory.CreateDirectory(FileLocation);
            }
            UserLocation = RootLocation + "/Users";
            if (!Directory.Exists(UserLocation))
            {
                Directory.CreateDirectory(UserLocation);
            }
            KeyLocation = RootLocation + "/Keys";
            if (!Directory.Exists(KeyLocation))
            {
                Directory.CreateDirectory(KeyLocation);
            }
        }
        #endregion
        #region FileSystem
        public static FileStream? GetFilebyFilename(string FileName) //Please dont return your Files from a Database or something in kind of as a byte[] or string its Filling up your Server Ram with each Request till its full
        {
            if (!File.Exists(FileLocation + "/" + FileName)) return null;
            return File.OpenRead(FileLocation + "/" + FileName);
        }  
        public static void UploadFile(string FileName, string Base64)
        {
            File.WriteAllBytes(FileLocation + "/" + FileName, Convert.FromBase64String(Base64));
        }
        public static bool FileExist(string FileName)
        {
            return File.Exists(FileLocation + "/" + FileName);
        }
        public static string[] GetAllFiles()
        {
            return Directory.GetFiles(FileLocation).Select(x=> x.Split('\\').Last()).ToArray();
        }
        public static void DeleteFile(string FileName)
        {
            if (File.Exists(FileLocation+"/"+FileName))
            {
                File.Delete(FileLocation+"/"+FileName);
            }
        }
        #endregion
        #region UserSystem
        static string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static Random random = new Random();
        public static string CreateRegisterKey()
        {
            string result = string.Empty;
            for (int i = 0; i < 20; i++)
            {
                result = result + chars[random.Next(chars.Length)];
            }
            if (File.Exists(KeyLocation + "/" + result))
            {
                return CreateRegisterKey();
            }
            File.Create(KeyLocation + "/" + result).Close();
            return result;
        }
        public static string Register(string Username, string Password, string RegisterKey)
        {
            if (string.IsNullOrWhiteSpace(Username)) return "Username is Null";
            if (string.IsNullOrWhiteSpace(Password)) return "Password is Null";
            Username = Username.ToLower();
            if (File.Exists(UserLocation+"/"+Username+".txt"))
            {
                return "This Username already Exist!";
            }
            if (!File.Exists(KeyLocation+"/"+RegisterKey))
            {
                return "Given key dont Exist!";
            }
            else
            {
                File.Delete(KeyLocation+"/"+RegisterKey);
            }
            File.Create(UserLocation + "/" + Username+".txt").Close();
            var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Password));
            string stringhash = string.Empty;
            foreach (byte b in hash)
                stringhash += b.ToString("x2");
            File.WriteAllText(UserLocation + "/" + Username + ".txt", stringhash);
            return $"Succesfully Registered {Username}!";
        }
        public static bool TryLogin(string Username, string Password)
        {
            if (string.IsNullOrWhiteSpace(Username)) return false;
            if (string.IsNullOrWhiteSpace(Password)) return false;
            Username = Username.ToLower();
            if (!File.Exists(UserLocation + "/" + Username + ".txt"))
            {
                return false;
            }
            var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Password));
            string stringhash = string.Empty;
            foreach (byte b in hash)
                stringhash += b.ToString("x2");
            string stringhash2 = File.ReadAllText(UserLocation + "/" + Username + ".txt");
            return stringhash == stringhash2;
        }
        #endregion
        #region Example
        static FileStream? DataBaseExample(string FileName)
        {
            if (!File.Exists(FileLocation + "/" + FileName))
            {
                //Cache(YourDBMethod(FileName), FileName);
            }
            FileStream stream = File.OpenRead(FileLocation + "/" + FileName);
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] DBHash = ExampleDBgetHashMethod(FileName);
            if (md5.ComputeHash(stream) != DBHash)
            {
                md5.Clear(); //Cleanup
                stream.Dispose(); //Cleanup
                GC.Collect(); //Cleanup
                //Cache(YourDBMethod(FileName), FileName);
                return DataBaseExample(FileName);
            }
            else
            {
                md5.Clear(); //Cleanup
                return stream;
            }
        }
        static byte[] ExampleDBgetHashMethod(string Filename)
        {
            return new byte[1];
        }
        static void Cache(ref byte[] data, string FileName) //A Example how you coud get the File from the DataBase and Save it on the Disk
        {
            File.WriteAllBytes(FileLocation + "/" + FileName, data);
            data = new byte[0]; //CleanUp
        }
        #endregion
    }
}
