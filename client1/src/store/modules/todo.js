import axios from 'axios'

// initial state
const state = () => ({
  items: [],
})

// getters
const getters = {
  // this.$store.getters["todo/todoCount"]
  todoCount: (state) => state.items.length,
  todoCount2(state) {
    return state.items.length
  },
  allItems(state) {
    return state.items
  },
}

// mutations は非同期が許されない。値の変更をするから
const mutations = {
  addToDoItem(state, item) {
    state.items.push(item)
  },
  deleteToDoItem(state, id) {
    state.items = state.items.filter((n) => n.id !== id)
  },
  setToDoItems(state, items) {
    state.items = items
  },
}

// actions は非同期が許される
const actions = {
  // this.$store.dispatch('todo/AddTodoItem', {email: email, context: context, token: token})
  async addTodoItem({ commit }, { email, context, token }) {
    const params = new URLSearchParams()
    params.append('Context', context) // 渡したいデータ分だけappendする
    params.append('email', email)

    const apiUrl = process.env.VUE_APP_API_URL + 'todo'
    var res = await axios.post(apiUrl, params, {
      headers: {
        Authorization: `Bearer ${token}`, // send the access token through the 'Authorization' header
      },
    })

    // id を返してもらう必要がある
    commit('addToDoItem', res.data)
  },
  // this.$store.dispatch('todo/deleleteTodoItem', {email: email, id: id, token: token})
  async deleleteTodoItem({ commit }, { email, id, token }) {
    const params = new URLSearchParams()
    params.append('email', email)

    // delete でformパラメータを渡す場合は、下記のようにする
    const apiUrl = process.env.VUE_APP_API_URL + `todo/${id}`
    var res = await axios.delete(apiUrl, {
      data: params,
      headers: {
        Authorization: `Bearer ${token}`, // send the access token through the 'Authorization' header
      },
    })

    console.log(res)
    commit('deleteToDoItem', id)
  },
  // this.$store.dispatch('todo/getTodoList', {email: email, token: token})
  async getTodoList({ commit }, { email, token }) {
    /*
  var config = {
    headers: { 'X-Api-Key': 'test' },
    withCredentials: true, // 同一ドメインだと自動で cookie を送信するらしい
    data: {}, // これがないとダメらしい
  }*/

    // await を使って例外処理を行うとうまくデバッグできない。。。
    // axios.defaultに値を設定すると、グローバル設定となり全てのリクエストに反映される。
    const apiUrl = process.env.VUE_APP_API_URL + 'todo'
    var res = await axios
      .get(apiUrl, {
        params: {
          email: email,
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
      commit('setToDoItems', res.data)
    }
  },
  // this.$store.dispatch('todo/updateTodoList', {email: email, token: token, todo: todo})
  async updateTodoItem({ commit }, { email, token, todo }) {
    const params = new URLSearchParams()
    params.append('email', email)
    params.append('status', todo.status)
    params.append('context', todo.context)

    // put でformパラメータを渡す場合は、下記のようにする
    const apiUrl = process.env.VUE_APP_API_URL + `todo/${todo.id}`
    var res = await axios.put(apiUrl, params, {
      headers: {
        Authorization: `Bearer ${token}`, // send the access token through the 'Authorization' header
      },
    })

    console.log(commit)
    console.log(res)
  },
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations,
}
