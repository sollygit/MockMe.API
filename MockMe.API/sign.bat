::Signing files
signtool sign /a /v "%1%2.exe"
signtool sign /a /v "%1%2.dll"
signtool sign /a /v "%1MockMe.Model.dll"
signtool sign /a /v "%1MockMe.Common.dll"
signtool sign /a /v "%1MockMe.Repository.dll"
signtool sign /a /v "%1Swashbuckle.AspNetCore.Swagger.dll"
signtool sign /a /v "%1Swashbuckle.AspNetCore.SwaggerGen.dll"
signtool sign /a /v "%1Swashbuckle.AspNetCore.SwaggerUI.dll"
signtool sign /a /v "%1Newtonsoft.Json.dll"
signtool sign /a /v "%1AutoMapper.dll"
echo Signing files success.