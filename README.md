# auth0 + spa sample

Auth0 と Vue.js + .NET Core web api を使ったサンプルです。
とりあえず VSCode の最新版を入れとくこと。

## Auth0 のアカウントを作り設定を行う

## バックエンドの API を起動する

Azure Cosmos DB を使っているのでまずはそれを作る。
データモデルはコア(SQL)を指定する。
プログラムでデータベース、コンテナは作成するので作るだけでよい。
作った Cosmos DB の エンドポイントと API キーと忘れないように。

.NET Core のシークレット機能を使って Cosmos DB への接続情報をもらっているので下記の初期化を行う

```
cd backend
dotnet user-secrets set "CosmosDb:EndpointUri" "input your endpoint uri"
dotnet user-secrets set "CosmosDb:PrimaryKey" "input your primary api key"
```

VSCode のデバッグから .NET Core Launch (web) (backend) を選択して実行する。
ソースがビルドされてプログラムが起動すると思う。

## フロントエンドを起動する

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
