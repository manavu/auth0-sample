import Vue from 'vue'
import VueRouter from 'vue-router'
import Home from '../views/Home.vue'
import ToDo from '../views/ToDo.vue'
import Profile from '../views/Profile.vue'
import Test from '../views/Test.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home,
  },
  {
    path: '/todo',
    name: 'ToDo',
    component: ToDo,
  },
  {
    path: '/profile',
    name: 'Profile',
    component: Profile,
  },
  {
    path: '/test',
    name: 'Test',
    component: Test,
  },
]

const router = new VueRouter({
  mode: 'history',
  routes,
})

export default router
