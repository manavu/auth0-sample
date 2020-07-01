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
        <p>total count: {{ this.todoCount }}</p>
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
      context: '',
    }
  },
  computed: {
    // mapState はオブジェクトを返す、そのため他のプロパティとマージするためにオブジェクトスプレット演算子 ... を使う
    // mapState メソッド経由でメソッドを作ることでプロキシ的な役割になるのかも
    ...mapState({
      items: (state) => state.todo.items,
    }),
    todoCount() {
      return this.$store.getters['todo/todoCount']
    },
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
  async mounted() {
    // このタイミングでは$auth がないっぽい。なので下記の処理は失敗する
    /*
    // ユーザー情報が入っていない、jwt が返ってくる
    const token = await this.$auth.getTokenSilently()
    // ユーザー情報を取得
    const claims = await this.$auth.getIdTokenClaims()

    // dispatch も非同期なメソッド
    await this.$store.dispatch('todo/getTodoList', {
      context: this.context,
      email: claims.email,
      token: token,
    })*/
  },
  methods: {
    // アロー演算子で書くと this が undefined
    // onAddTaskHandler: async (e) => {
    async onAddTaskHandler(e) {
      e.preventDefault()

      // ユーザー情報が入っていない、jwt が返ってくる
      const token = await this.$auth.getTokenSilently()
      // ユーザー情報を取得
      const claims = await this.$auth.getIdTokenClaims()

      // dispatch も非同期なメソッド
      await this.$store.dispatch('todo/addTodoItem', {
        context: this.context,
        email: claims.email,
        token: token,
      })

      this.context = ''
    },
    async onDelTaskHandler(e, id) {
      e.preventDefault()

      const token = await this.$auth.getTokenSilently()
      const claims = await this.$auth.getIdTokenClaims()

      await this.$store.dispatch('todo/deleleteTodoItem', {
        id: id,
        email: claims.email,
        token: token,
      })
    },
    async onGetTaskHandler(e) {
      e.preventDefault()

      const token = await this.$auth.getTokenSilently()
      const claims = await this.$auth.getIdTokenClaims()

      await this.$store.dispatch('todo/getTodoList', {
        email: claims.email,
        token: token,
      })
    },
  },
}
</script>
