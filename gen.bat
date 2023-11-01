rmdir Entity /s /q
rmdir Contexts /s /q
dotnet ef dbcontext scaffold "Data Source=ADMIN;Initial Catalog=SHOU;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Encrypt=False;Trust Server Certificate=False;Command Timeout=0" Microsoft.EntityFrameworkCore.SqlServer -o Entity -c SHOUContext -f --context-dir Contexts

timeout /t 1000