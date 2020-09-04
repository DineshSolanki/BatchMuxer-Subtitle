SET currentPath=%~dp0
Set filePath=%currentPath%BatchMuxer_Subtitle_Console.exe
@Reg Add "HKCR\Directory\shell\Merge Subtitles" /VE /D "Merge Subtitles" /F >Nul
@Reg Add "HKCR\Directory\shell\Merge Subtitles\command" /VE /D "\"%filePath%\" \"%%1\"" /F >Nul

@Reg Add "HKCR\Directory\Background\shell\Merge Subtitle" /VE /D "Merge Subtitles" /F >Nul
@Reg Add "HKCR\Directory\Background\shell\Merge Subtitle\command" /VE /D "\"%filePath%\" \"%%V\"" /F >Nul
