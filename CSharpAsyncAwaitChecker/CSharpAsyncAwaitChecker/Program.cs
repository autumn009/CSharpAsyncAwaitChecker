using System.Reflection.Metadata.Ecma335;

if ( args.Length != 1)
{
    Console.WriteLine("usage: CShapAsyncAwaitChecker DIR_NAME");
    return;
}

/// 例外文字列テーブル
/// 警告の例外とする文字列をここで定義する
string[] customExceptTable =
{
    "/* DIABLE ASYNC WARN */",
    "new SimpleMenuItem(",
    "ProcedureAsync = async",
    "MethodAsync = async",
    "Task<WeatherForecast[]>",
};

foreach (var fullpath in Directory.EnumerateFiles(args[0],"*.cs", SearchOption.AllDirectories))
{
    if (fullpath.Contains(".g.")) continue;
    checkOneFile(fullpath);
}

void checkOneFile(string fullpath)
{
    using var reader = new StreamReader(fullpath);
    for (int lineNumber = 1; ; lineNumber++)
    {
        var s = reader.ReadLine();
        if (s == null) break;
        // コメント行は読み飛ばす
        if (s.TrimStart().StartsWith("//")) continue;
        // 禁止文字列を含む行は読み飛ばす
        foreach (var item in customExceptTable)
        {
            if (s.Contains(item)) goto nextLine;
        }

        // async check
        int from = 0;
        for (; ; )
        {
            if (from >= s.Length) break;
            int index1 = s.IndexOf("async", from);
            if (index1 >= 0)
            {
                from = index1 + 5;
                char before = ' ', after = ' ';
                if (index1 > 0) before = s[index1 - 1];
                if (index1 + 5 < s.Length) after = s[index1 + 5];
                if (!char.IsAsciiLetter(before) && !char.IsAsciiLetter(after))
                {
                    if (s.IndexOf("=>", from) >= 0) goto nextLine;  // OK goto next step
                    int index2 = s.IndexOf("Async", from);
                    if (index2 >= 0)
                    {
                        char after2 = ' ';
                        if (index2 + 5 < s.Length) after2 = s[index2 + 5];
                        if (!char.IsAsciiLetter(after2)) goto nextLine;  // OK goto next line
                    }
                    Console.WriteLine($"{fullpath}:{lineNumber} {s}");
                }
            }
            else
                goto nextStep;
        }
        // await check
        nextStep:
        int index3 = s.IndexOf("Async");
        if (index3 >= 0)
        {
            if (s.Contains("=>")) goto nextLine;    // IT's safe
            char after2 = ' ';
            if (index3 + 5 < s.Length) after2 = s[index3 + 5];
            if (!char.IsAsciiLetter(after2))
            {
                int index2 = s.IndexOf("await");
                if (index2 >= 0)
                {
                    char before = ' ', after = ' ';
                    if (index2 > 0) before = s[index2 - 1];
                    if (index2 + 5 < s.Length) after = s[index2 + 5];
                    if (!char.IsAsciiLetter(before) && !char.IsAsciiLetter(after))
                    {
                        continue;     // OK goto next line
                    }
                }
            }
            Console.WriteLine($"{fullpath}:{lineNumber} {s}");
        }
    nextLine:;
    }
}
