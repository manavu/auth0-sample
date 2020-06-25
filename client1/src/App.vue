<template>
  <div id="app">
    <b-navbar toggleable="sm" type="dark" variant="dark">
      <b-navbar-brand href="#">SPA + Auth0 ToDo Sample</b-navbar-brand>
      <b-navbar-toggle target="navbar-toggle-collapse"> </b-navbar-toggle>
      <b-collapse id="navbar-toggle-collapse" is-nav>
        <b-navbar-nav>
          <b-nav-item v-if="!$auth.loading && $auth.isAuthenticated" to="/todo"
            >ToDo</b-nav-item
          >
          <b-nav-item
            v-if="!$auth.loading && $auth.isAuthenticated"
            to="/profile"
            >Profile</b-nav-item
          >
          <b-nav-item
            v-if="!$auth.loading && !$auth.isAuthenticated"
            href="#"
            v-on:click.prevent.stop="login"
            >Log in</b-nav-item
          >
          <b-nav-item
            v-if="!$auth.loading && $auth.isAuthenticated"
            href="#"
            v-on:click.prevent.stop="logout"
            >Log out</b-nav-item
          >
        </b-navbar-nav>
      </b-collapse>
    </b-navbar>
    <router-view />
  </div>
</template>

<script>
export default {
  name: 'app',
  methods: {
    // Log the user in
    login() {
      this.$auth.loginWithRedirect()
    },
    // Log the user out
    logout() {
      this.$auth.logout({
        returnTo: window.location.origin,
      })
    },
  },
}
</script>

<style lang="scss">
@import '../node_modules/bootstrap/scss/bootstrap.scss';
@import '../node_modules/bootstrap-vue/src/index.scss';
</style>
