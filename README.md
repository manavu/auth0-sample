# auth0 + spa sample

Auth0 と Vue.js + .NET Core web api を使ったサンプルです。
とりあえず VSCode の最新版を入れとくこと。

## Auth0 のアカウントを作り設定を行う

Applications と APIs にそれぞれアプリ用の Application と API を作る。
設定は割愛

## バックエンドの API を起動する

Azure Cosmos DB を使っているのでまずはそれを作る。
データモデルはコア(SQL)を指定する。
プログラムでデータベース、コンテナは作成するので作るだけでよい。
作った Cosmos DB の エンドポイントと API キーと忘れないように。

.NET Core のシークレット機能を使って Auth0 と Cosmos DB への接続情報をもらっているので下記の初期化を行う

```
cd backend
dotnet user-secrets set "CosmosDb:EndpointUri" "input your endpoint uri"
dotnet user-secrets set "CosmosDb:PrimaryKey" "input your primary api key"
dotnet user-secrets set "Auth0:Domain" "input your auth0 application domain"
dotnet user-secrets set "Auth0:Audience" "input your auth0 APIs Identifier"
```

VSCode のデバッグから .NET Core Launch (web) (backend) を選択して実行する。
ソースがビルドされてプログラムが起動すると思う。

## フロントエンドを起動する

auth_config.json ファイルに記載されている情報を変更する。
このやり方はよくないのでどっかで変えられるようにしよう('A`)

clinet1 のディレクトリから下記のコマンドを実行する

```
npm install
npm run serve
```

または Vue の GUI を起動してそこから操作しても実行できます
```
vue ui
```

VSCode のデバッグから vuejs: chrome (client1) を選択して実行する
