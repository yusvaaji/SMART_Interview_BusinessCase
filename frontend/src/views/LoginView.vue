<script setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  import { mdiAccount, mdiAsterisk } from '@mdi/js'
  import SectionFullScreen from '@/components/SectionFullScreen.vue'
  import CardBox from '@/components/CardBox.vue'
  import FormCheckRadio from '@/components/FormCheckRadio.vue'
  import FormField from '@/components/FormField.vue'
  import FormControl from '@/components/FormControl.vue'
  import BaseButton from '@/components/BaseButton.vue'
  import BaseButtons from '@/components/BaseButtons.vue'
  import LayoutGuest from '@/layouts/LayoutGuest.vue'
  import { useMainStore } from '@/stores/main.js'

  const email = ref('test@admin.com')
  const password = ref('admin123')
  const remember = ref(true)
  const router = useRouter()
  const mainStore = useMainStore()
  const error = ref('')

  const loginCons = async () => {
    try {
      await mainStore.login(email.value, password.value)
      console.log('Logged in')
      //this.$router.push('/dashboard') // Redirect to home or dashboard after successful login
      router.push('/dashboard') // Redirect to home or dashboard after successful login
     
    } catch (err) {
      console.error(err)
      error.value = 'Invalid email or password'
    }
  }
</script>

<template>
  <LayoutGuest>
    <SectionFullScreen v-slot="{ cardClass }" bg="purplePink">
      <CardBox :class="cardClass" is-form @submit.prevent="loginCons">
        <FormField label="Login" help="Please enter your login">
          <FormControl v-model="email"
                       :icon="mdiAccount"
                       name="login"
                       autocomplete="username" />
        </FormField>

        <FormField label="Password" help="Please enter your password">
          <FormControl v-model="password"
                       :icon="mdiAsterisk"
                       type="password"
                       name="password"
                       autocomplete="current-password" />
        </FormField>

        <FormCheckRadio v-model="remember"
                        name="remember"
                        label="Remember"
                        :input-value="true" />

        <template #footer>
          <BaseButtons>
            <BaseButton type="submit" color="info" label="Login" />
          </BaseButtons>
        </template>
        <div v-if="error" class="error">{{ error }}</div>
      </CardBox>
    
    </SectionFullScreen>
  </LayoutGuest>
</template>

<style scoped>
  .error {
    color: red;
    margin-top: 1rem;
  }
</style>
