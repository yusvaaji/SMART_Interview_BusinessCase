<template>
  <div>
    <div class="flex justify-between items-center my-4">
      <h2 class="text-xl font-bold">Construction Projects</h2>
      <router-link to="/manage-project" class="bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-700">Add Project</router-link>
    </div>
    <div v-if="projects.length">
      <div v-for="project in projects" :key="project.projectId" class="my-4">
        <ProjectCard :project="project" />
      </div>
    </div>
    <div v-else>
      <p>No projects found.</p>
    </div>
  </div>
</template>

<script setup>
  import { onMounted, computed } from 'vue'
  import { useMainStore } from '@/stores/main.js'
  import ProjectCard from '@/components/constructionproject/ProjectCard.vue'

  const mainStore = useMainStore()

  const projects = computed(() => mainStore.projects)

  onMounted(() => {
    mainStore.fetchProjects()
  })
</script>
