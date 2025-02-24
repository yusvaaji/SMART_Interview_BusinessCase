<template>
  <LayoutAuthenticated>
    <SectionMain>
      <SectionTitleLineWithButton :icon="mdiBallotOutline" title="Manage Project" main>
        <!--<BaseButton href="https://github.com/justboil/admin-one-vue-tailwind"
                    target="_blank"
                    :icon="mdiGithub"
                    label="Star on GitHub"
                    color="contrast"
                    rounded-full
                    small />-->
      </SectionTitleLineWithButton>
      <ConstructionProjectForm :project="project" />
    </SectionMain>
  </LayoutAuthenticated>
</template>

<script setup>
  import { ref, computed } from 'vue'
  import { useRoute } from 'vue-router'
  import { useMainStore } from '@/stores/main.js'
  import LayoutAuthenticated from '@/layouts/LayoutAuthenticated.vue'
  import SectionMain from '@/components/SectionMain.vue'
  import SectionTitleLineWithButton from '@/components/SectionTitleLineWithButton.vue'
  import ConstructionProjectForm from '@/components/constructionproject/ConstructionProjectForm.vue'

  // Get route and main store instances
  const route = useRoute()
  const mainStore = useMainStore()

  // Get the project ID from the route parameters
  const projectId = ref(route.params.id)

  // Fetch projects if they are not already loaded
  if (!mainStore.projects.length) {
    mainStore.fetchProjects()
  }

  // Get the project data based on the project ID
  const project = computed(() => {
    return projectId.value ? mainStore.projects.find(p => p.projectId === projectId.value) : {}
  })
</script>
