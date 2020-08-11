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
    meta: { requiresAuth: true },
  },
  {
    path: '/profile',
    name: 'Profile',
    component: Profile,
    meta: { requiresAuth: true },
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

router.beforeEach((to, from, next) => {
  if (to.matched.some((record) => !record.meta.requiresAuth)) {
    next()
  } else if (router.app.$auth.isAuthenticated) {
    next()
  } else {
    // router.app.$auth.login()
    next({ path: '/', query: { redirect: to.fullPath } })
  }
})

export default router
