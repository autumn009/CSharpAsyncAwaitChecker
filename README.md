# CSharpAsyncAwaitChecker
Check C# Source Codes about async and await
指定したディレクトリ配下のC#のソースコードに対して以下のチェックを行います。
対象は拡張子.csのファイルです。

・asyncキーワードが存在する行に、文字列"=>"またはAsyncで終わるキーワードが存在する
・Asyncで終わるキーワードが存在する行にawaitキーワードが存在する　(asyncキーワードが存在する行は除く)
