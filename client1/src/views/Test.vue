<template>
  <b-container fluid="md" class="test">
    <!-- <div class="row"> -->
    <b-row>
      <!-- <div class="col-md-2"> -->
      <b-col md="4">
        <b-alert show>Default Alert</b-alert>
      </b-col>
      <b-col md="8">
        <b-button block variant="primary">Block Level Button</b-button>
      </b-col>
    </b-row>
    <b-form-row>
      <b-form @submit="onSubmit" @reset="onReset" v-if="show">
        <b-form-group label="Email address:" label-for="input-1">
          <b-form-input
            id="input-1"
            v-model="form.email"
            type="email"
            placeholder="Enter email"
            required
          ></b-form-input>
        </b-form-group>
        <b-form-group label="Name:" label-for="input-2">
          <b-form-input
            id="input-2"
            v-model="form.name"
            type="text"
            placeholder="Enter name"
            required
          ></b-form-input>
        </b-form-group>
        <b-form-group label="Food:" label-for="input-3">
          <b-form-select
            id="input-3"
            v-model="form.food"
            :options="foods"
            required
          ></b-form-select>
        </b-form-group>
        <b-form-group label="Birthday:" label-for="input-4">
          <b-form-datepicker
            id="input-4"
            v-model="form.birthday"
            required
          ></b-form-datepicker>
        </b-form-group>
        <b-button type="submit" variant="primary">Submit</b-button>
        <b-button type="reset" variant="danger">Reset</b-button>
      </b-form>
    </b-form-row>
  </b-container>
</template>

<script>
// @ is an alias to /src
// import HelloWorld from '@/components/HelloWorld.vue'

export default {
  name: 'Test',
  components: {
    // HelloWorld,
  },
  data() {
    return {
      form: {
        email: '',
        name: '',
        food: null,
        birthday: null,
        checked: [],
      },
      foods: [
        { text: 'Select One', value: null },
        'Carrots',
        'Beans',
        'Tomatoes',
        'Corn',
      ],
      show: true,
    }
  },
  methods: {
    onSubmit(e) {
      e.preventDefault()
    },
    onReset(e) {
      e.preventDefault()

      this.form.email = ''
      this.form.name = ''
      this.form.food = null
      this.form.birthday = null
    },
  },
}
</script>
