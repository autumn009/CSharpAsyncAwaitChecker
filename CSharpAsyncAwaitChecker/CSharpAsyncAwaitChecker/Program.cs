﻿if( args.Length != 1)
{
    Console.WriteLine("usage: CShapAsyncAwaitChecker DIR_NAME");
    return;
}

foreach (var fullpath in Directory.EnumerateFiles(args[0],"*.cs"))
{
    checkOneFile(fullpath);
}

void checkOneFile(string fullpath)
{
    using var reader = new StreamReader(fullpath);
    for (int lineNumber=0; ; lineNumber++)
    {
        var s = reader.ReadLine();
        if (s == null) break;
        // async check
        int from = 0;
        for (; ; )
        {
            if (from >= s.Length) break;
            int index1 = s.IndexOf("async",from);
            if (index1 >= 0)
            {
                from = index1 + 5;
                char before = ' ', after = ' ';
                if (index1 > 0) before = s[index1 - 1];
                if (index1 + 5 < s.Length) after = s[index1 + 5];
                if (char.IsWhiteSpace(before) && char.IsWhiteSpace(after))
                {
                    if (s.IndexOf("=>", from) >= 0) break;  // OK goto next step
                    int index2 = s.IndexOf("Async", from);
                    if (index2 >= 0)
                    {
                        char after2 = ' ';
                        if (index2 + 5 < s.Length) after2 = s[index1 + 5];
                        if (char.IsWhiteSpace(after2)) continue;  // OK goto next line
                    }
                    Console.WriteLine($"{fullpath}:{lineNumber} {s}");
                }
            }
            else
                break;
        }
        // await check





    }
}