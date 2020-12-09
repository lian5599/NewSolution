
namespace Helper
{
    public static class File
    {
        /// <summary>
        /// Absolute path or relative path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Read(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        /// <summary>
        /// Absolute path or relative path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="contents"></param>
        public static void Write(string path, string contents)
        {
            System.IO.File.WriteAllText(path, contents);
        }

    }
}
