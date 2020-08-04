namespace backend
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpsPolicy;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Infras;
    using Microsoft.IdentityModel.Tokens;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Leave any code your app/template already has here and just add these lines:
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder
                        .WithOrigins("https://localhost:8080")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });

            // デフォルトではGDPRに準拠するために個人を特定する情報を例外に含めないのでそれを表示させる
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            var domain = $"https://{Configuration["Auth0:Domain"]}/";
            var audience = Configuration["Auth0:Audience"];
            // var secretKey = "9Teenct6h8ovmHtjM2D8-34VSo1IQ3x_Jbv0EhjgBx61-K-ooV-JhQy0immWOm6M";
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    // DER：バイナリーデータ
                    // PEM：DERをBase64エンコードして、ヘッダーとフッターをつけたもの
                    // base64url というものがある。これは + を - に、/ を _ に置き換えてURLで表現可能にする

                    // RS256 は公開鍵/秘密鍵の非対称での署名
                    // HS256 は共有鍵での署名

                    // RS256 の場合は公開鍵・秘密鍵の非対称での署名なので、公開鍵を取得する必要がある
                    // https://[your_domain].auth0.com/pem から公開鍵を取得できるが
                    // https://[your_domain].auth0.com/.well-known/jwks.json から関連する情報を取得し

                    // JWK を１行で説明すると、鍵のデータをJSON形式で表す仕様
                    // JWK の最初の鍵のデータは、該当するテナント用。二個目はテナントに対する署名
                    // なので、最初の鍵のデータを使うことになる

                    // n はモジュラスで、e が公開指数。この二つを使うことでプロバイダを初期化できる
                    // プロバイダが初期化出来たら、ヘッダーとペイロード

                    options.Authority = domain;
                    options.Audience = audience;
                    // 指定しない場合は、domain +  ".well-known/openid-configuration" になる
                    // この中に jwks_uri がある。
                    // 下記のサイトはこのモジュールのソースの紹介っぽい
                    // https://www.cnblogs.com/holdengong/p/12549744.html
                    // options.RequireHttpsMetadata = true;
                    // options.MetadataAddress = domain + ".well-known/jwks.json";
                    // MetadataAddress を設定して、ValidateIssuer と ValidateAudience を flase にすると認証が失敗する
                    options.SaveToken = true;   // default true
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        NameClaimType = ClaimTypes.NameIdentifier,
                        // ValidateIssuer = true,      // トークンを作成したサーバーを検証する
                        RequireSignedTokens = true, // 署名付きトークン必須
                        // ValidateIssuerSigningKey = true,    // 署名を検証する
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        // HS256 での改ざん検知の場合は共有鍵を設定
                        // IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

                        /*
                        // 証明書付きの公開鍵
                        var x509 = X509Certificate.CreateFromCertFile(@"C:\Users\manabu\Downloads\manavu.cer");
                        // 公開鍵を取得
                        var der = x509.GetPublicKey();

                        // 公開鍵から非対称の暗号化のプロバイダーを取得
                        var provider = new RSACryptoServiceProvider();
                        provider.ImportRSAPublicKey(der, out var bytesRead);
                        IssuerSigningKey = new RsaSecurityKey(provider),
                        */
                        /*
                        SignatureValidator = (tokenString, parameters) =>
                        {
                            // これは呼び出される。独自で署名の確認を行う方法なのかも
                            var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(tokenString);
                            
                            // 証明書付きの公開鍵
                            //var x509 = X509Certificate.CreateFromCertFile(@"C:\Users\manabu\Downloads\manavu.cer");
                            // 公開鍵を取得
                            //var der = x509.GetPublicKey();

                            // 公開鍵から非対称の暗号化のプロバイダーを取得
                            //var provider = new RSACryptoServiceProvider();
                            //provider.ImportRSAPublicKey(der, out var bytesRead);
                            
                            // モジュラス
                            var n = "z-WTKJtHB-dxk_XZ2oVfzdGaLFZn40r6HIe6q6mtqpSvK2SSdSTjsSYBPwfL3lzCwBqzEzzdONXau3S7uNNCd5K64mRRxM7Nq5tzE4_ej5-pfNTMYCDGU86wFWFf6qfRwonySxsvfpyeiM0uBJ9visIenO2eeCs63Bn4HAhfgoS8JTFIRl13227z5LPYxAHE6_PdLXhqffl0l7kxTOrtxv1HVzonECEwCCcifhSlSG5ldc-6BRJmOr-Tgsy0U1xR4I9NvalLr-fUZ8Mx4UVHBdVA6Z2vXj0pK_z3iy0LqqUIwsdC_A5s_NgnWP-afA0iAyJUBc2sM2o3faDyy1dkxw";
                            // 公開指数
                            var e = "AQAB";

                            var rsaParameters = new RSAParameters();
                            rsaParameters.Modulus = Base64UrlEncoder.DecodeBytes(n);
                            rsaParameters.Exponent = Base64UrlEncoder.DecodeBytes(e);

                            var provider = new RSACryptoServiceProvider();
                            provider.ImportParameters(rsaParameters);

                            var data = Encoding.ASCII.GetBytes(jwt.RawHeader + "." + jwt.RawPayload);
                            var signature = Base64UrlEncoder.DecodeBytes(jwt.RawSignature);
                            var ret = provider.VerifyData(data, "SHA256", signature);

                            return jwt;
                        }*/
                    };
                });

            /*
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,  // トークンを作成したサーバーを検証する
            ValidateAudience = true,    // トークンの受信人が受信を許可されているかを確認する
            ValidateLifetime = true,    // トークンの期限が切れておらず、Issuerの署名鍵が有効であることをチェックする
            ValidateIssuerSigningKey = true,    // 受信したトークンを署名するために使用された鍵が信頼された鍵のリストに入っていることを検証する
            ValidIssuer = Configuration["Jwt:Issuer"],
            ValidAudience = Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
          };
            */

            // 承認時に特定の許可を得ているようにする場合
            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:messages", policy => policy.Requirements.Add(new HasScopeRequirement("read:messages", domain)));
            });

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Leave any code your app/template already has here and just add the line:
            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
