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
              <th>No.</th>
              <th>内容</th>
              <th>作成日時</th>
              <th>状態</th>
              <th>操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in items" v-bind:key="item.id">
              <td>{{ index + 1 }}</td>
              <td>
                <input
                  type="input"
                  class="form-control"
                  v-model="item.context"
                />
              </td>
              <td>{{ item.createAt | moment }}</td>
              <td>
                <select v-model="item.status">
                  <option
                    v-for="option in statusOptions"
                    v-bind:key="option.value"
                    v-bind:value="option.value"
                    >{{ option.text }}</option
                  >
                </select>
              </td>
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
    <div class="row">
      <div class="col-md-12">
        <button class="btn btn-primary" v-on:click="onUpdateTaskHandler">
          保存
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import MyMixin from './../mixins/index'
import { mapState, mapGetters } from 'vuex'

export default {
  name: 'ToDo',
  mixins: [MyMixin],
  data() {
    // data はページが切り替わると消えるので、vuex を使って永続化する必要がある
    return {
      context: '',
      statusOptions: [
        { text: '新規', value: 'New' },
        { text: '実行中', value: 'Active' },
        { text: '完了', value: 'Done' },
      ],
    }
  },
  computed: {
    // mapState はオブジェクトを返す、そのため他のプロパティとマージするためにオブジェクトスプレット演算子 ... を使う
    // mapState メソッド経由でメソッドを作ることでプロキシ的な役割になるのかも
    ...mapState({
      items: (state) => state.todo.items,
    }),
    /*
    todoCount() {
      return this.$store.getters['todo/todoCount']
    },*/
    // 上の書き方と同じ結果になる
    ...mapGetters('todo', ['todoCount']),
  },
  /*watch: {
    readOnlyItems(values) {
      this.items = values
    },
  },*/
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
    async onUpdateTaskHandler(e) {
      e.preventDefault()

      const token = await this.$auth.getTokenSilently()
      const claims = await this.$auth.getIdTokenClaims()

      await Promise.all(
        this.items.map(async (item) => {
          await this.$store.dispatch('todo/updateTodoItem', {
            email: claims.email,
            token: token,
            todo: item,
          })
        })
      )
    },
  },
}
</script>
