module.exports = {
  configureWebpack: {
    devtool: 'inline-source-map',
    devServer: {
      https: true,
      // port: 3000,
      proxy: {
        '/api': {
          target: 'https://localhost:5001/',
          // pathRewrite: { '^/api': '' }, // rewrite
        },
      },
    },
  },
}
