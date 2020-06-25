import axios from 'axios'

// initial state
const state = () => ({
  items2: [{ id: 1, context: '初期のデータ' }],
})

// getters
const getters = {
  // this.$store.getters.todoCount
  todoCount(state) {
    return state.items.count()
  },
}

// mutations は非同期が許されない。値の変更をするから
const mutations = {
  addToDoItem(state, item) {
    state.items2.push(item)
  },
  deleteToDoItem(state, id) {
    state.items2 = state.items2.filter((n) => n.id !== id)
  },
  setToDoItems(state, items) {
    state.items2 = items
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
    console.log(res)
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
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations,
}
