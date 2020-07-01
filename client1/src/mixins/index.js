import Moment from 'moment'

// mixin の機能を使って複数のモジュールで処理を共有できる

export default {
  filters: {
    // テンプレートで使用可能な変換処理などを定義できる
    moment(date) {
      return Moment(date).format('YYYY/MM/DD HH:mm')
    },
  },
}
