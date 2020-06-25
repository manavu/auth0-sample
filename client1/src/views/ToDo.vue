<template>
  <div class="container todo">
    <form class="form-inline" @submit="onAddTaskHandler" method="post">
      <div class="form-group">
        <label class="col-md-4" for="context">内容</label>
        <input
          type="text"
          class="form-control col-md-8"
          id="context"
          v-model="context"
          value
        />
      </div>
      <div class="form-group">
        <div class="col-md-12">
          <button class="btn btn-primary" type="submit">追加</button>
        </div>
      </div>
    </form>
    <div class="row">
      <div class="col-md-12">
        <button class="btn btn-primary" v-on:click="onGetTaskHandler">
          一覧取得
        </button>
      </div>
    </div>
    <div class="row">
      <div class="col-md-12">
        <table class="table table-striped">
          <thead>
            <tr>
              <th>ID</th>
              <th>内容</th>
              <th>作成日時</th>
              <th>操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in items" v-bind:key="item.id">
              <td>{{ item.id }}</td>
              <td>{{ item.context }}</td>
              <td>{{ item.createAt | moment }}</td>
              <td>
                <button
                  class="btn btn-danger"
                  v-on:click="onDelTaskHandler($event, item.id)"
                >
                  削除
                </button>
              </td>
            </tr>
            <tr v-for="item in items2" v-bind:key="item.id">
              <td>{{ item.id }}</td>
              <td>{{ item.context }}</td>
              <td>a</td>
              <td>a</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import Moment from 'moment'
import { mapState } from 'vuex'

export default {
  name: 'ToDo',
  data() {
    // data はページが切り替わると消えるので、vuex を使って永続化する必要がある
    return {
      items: [],
      context: '',
    }
  },
  computed: {
    // mapState はオブジェクトを返す、そのため他のプロパティとマージするためにオブジェクトスプレット演算子 ... を使う
    // mapState メソッド経由でメソッドを作ることでプロキシ的な役割になるのかも
    ...mapState({
      items2: (state) => state.todo.items2,
    }),
  },
  filters: {
    // テンプレートで使用可能な変換処理などを定義できる
    moment(date) {
      return Moment(date).format('YYYY/MM/DD HH:mm')
    },
  },
  created: function() {
    // このタイミングでは$auth がないっぽい
    /*
    let token = ''
    let claims = []

    // ユーザー情報が入っていない、jwt が返ってくる
    this.$auth.getTokenSilently().then((res) => (token = res))
    // ユーザー情報を取得
    this.$auth.getIdTokenClaims().then((res) => (claims = res))

    this.$store.dispatch('todo/getTodoList', {
      email: claims.email,
      token: token,
    })*/
  },
  methods: {
    async onAddTaskHandler(e) {
      e.preventDefault()

      // ユーザー情報が入っていない、jwt が返ってくる
      const token = await this.$auth.getTokenSilently()
      // ユーザー情報を取得
      const claims = await this.$auth.getIdTokenClaims()

      //const token = await this.$auth.getTokenSilently({scope:"read:message"});
      // このようにオプションを渡してもうまく行かない。というか失敗する

      const params = new URLSearchParams()
      params.append('Context', this.context) // 渡したいデータ分だけappendする
      params.append('email', claims.email)

      const apiUrl = process.env.VUE_APP_API_URL + 'todo'
      var res = await this.$axios.post(apiUrl, params, {
        headers: {
          Authorization: `Bearer ${token}`, // send the access token through the 'Authorization' header
        },
      })

      this.items.push(res.data)

      this.context = ''
    },
    async onDelTaskHandler(e, id) {
      e.preventDefault()

      // jwt が返ってくる
      const token = await this.$auth.getTokenSilently()
      const claims = await this.$auth.getIdTokenClaims()

      const params = new URLSearchParams()
      params.append('email', claims.email)

      // delete でformパラメータを渡す場合は、下記のようにする
      const apiUrl = process.env.VUE_APP_API_URL + `todo/${id}`
      var res = await this.$axios.delete(apiUrl, {
        data: params,
        headers: {
          Authorization: `Bearer ${token}`, // send the access token through the 'Authorization' header
        },
      })

      // 削除した要素を消す
      this.items = this.items.filter((m) => m.id != id)

      console.log(res)
    },
    async onGetTaskHandler(e) {
      e.preventDefault()

      // jwt が返ってくる
      const token = await this.$auth.getTokenSilently()
      const claims = await this.$auth.getIdTokenClaims()

      /*
      var config = {
        headers: { 'X-Api-Key': 'test' },
        withCredentials: true, // 同一ドメインだと自動で cookie を送信するらしい
        data: {}, // これがないとダメらしい
      }*/

      // await を使って例外処理を行うとうまくデバッグできない。。。
      // axios.defaultに値を設定すると、グローバル設定となり全てのリクエストに反映される。
      const apiUrl = process.env.VUE_APP_API_URL + 'ToDo'
      var res = await this.$axios
        .get(apiUrl, {
          params: {
            email: claims.email,
          },
          headers: {
            Authorization: `Bearer ${token}`, // send the access token through the 'Authorization' header
          },
        })
        .catch((e) => {
          console.log(e)
        })

      if (res.status == 200) {
        // データに反映させる
        this.$set(this, 'items', res.data)
      }

      /*
      this.$store.dispatch('todo/getTodoList', {
        email: claims.email,
        token: token,
      })*/
    },
  },
}
</script>
