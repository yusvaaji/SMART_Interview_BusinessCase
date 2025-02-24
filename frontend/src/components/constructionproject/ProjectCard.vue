<template>
  <div class="border rounded p-4 shadow">
    <h3 class="text-lg font-bold">{{ project.projectName }}</h3>
    <p><strong>Location:</strong> {{ project.projectLocation }}</p>
    <p><strong>Stage:</strong> {{ project.projectStage }}</p>
    <p><strong>Category:</strong> {{ project.projectCategory }}</p>
    <p><strong>Start Date:</strong> {{ project.projectConstructionStartDate }}</p>
    <p><strong>Details:</strong> {{ project.projectDetails }}</p>
    <div class="flex justify-end mt-4">
      <router-link :to="{ name: 'ManageProject', params: { id: project.projectId } }" class="bg-yellow-500 text-white py-2 px-4 rounded hover:bg-yellow-700 mr-2">Edit</router-link>
      <button @click="deleteProject(project.projectId)" class="bg-red-500 text-white py-2 px-4 rounded hover:bg-red-700">Delete</button>
    </div>
  </div>
</template>

<script setup>
  import { defineProps } from 'vue'
  import { useMainStore } from '@/stores/main.js'

  const props = defineProps({
    project: {
      type: Object,
      required: true
    }
  })

  const mainStore = useMainStore()

  const deleteProject = (projectId) => {
    mainStore.deleteProject(projectId)
  }
</script>
