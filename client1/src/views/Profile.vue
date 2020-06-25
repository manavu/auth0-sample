<template>
  <div class="container">
    <img :src="$auth.user.picture" />
    <h2>{{ $auth.user.name }}</h2>
    <p>{{ $auth.user.email }}</p>
    <div class="row">
      <pre>{{ JSON.stringify($auth.user, null, 2) }}</pre>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Profile',
  methods: {
    async onClickHander() {
      // jwt が帰ってくる
      const claims = await this.$auth.getIdTokenClaims()
      console.log(claims)
    },
  },
}
</script>
