<template>
  <div>
    <form @submit.prevent="handleSubmit">
      <div>
        <label for="projectName" class="block text-sm font-medium text-gray-700">Project Name</label>
        <input v-model="project.projectName" type="text" id="projectName" class="mt-1 block w-full" required maxlength="200">
      </div>
      <div class="mt-4">
        <label for="projectLocation" class="block text-sm font-medium text-gray-700">Project Location</label>
        <input v-model="project.projectLocation" type="text" id="projectLocation" class="mt-1 block w-full" required maxlength="500">
      </div>
      <div class="mt-4">
        <label for="projectStage" class="block text-sm font-medium text-gray-700">Project Stage</label>
        <select v-model="project.projectStage" id="projectStage" class="mt-1 block w-full" required>
          <option value="Planning">Planning</option>
          <option value="Design">Design</option>
          <option value="Construction">Construction</option>
          <option value="Completed">Completed</option>
        </select>
      </div>
      <div class="mt-4">
        <label for="projectCategory" class="block text-sm font-medium text-gray-700">Project Category</label>
        <select v-model="project.projectCategory" id="projectCategory" class="mt-1 block w-full" required>
          <option value="Education">Education</option>
          <option value="Health">Health</option>
          <option value="Office">Office</option>
          <option value="Others">Others</option>
        </select>
        <div v-if="project.projectCategory === 'Others'" class="mt-2">
          <input v-model="project.otherCategory" type="text" class="mt-1 block w-full" placeholder="Specify other category">
        </div>
      </div>
      <div class="mt-4">
        <label for="projectConstructionStartDate" class="block text-sm font-medium text-gray-700">Construction Start Date</label>
        <input v-model="project.projectConstructionStartDate" type="date" id="projectConstructionStartDate" class="mt-1 block w-full" required>
      </div>
      <div class="mt-4">
        <label for="projectDetails" class="block text-sm font-medium text-gray-700">Project Details</label>
        <textarea v-model="project.projectDetails" id="projectDetails" class="mt-1 block w-full" required maxlength="2000"></textarea>
      </div>
      <div class="mt-6">
        <button type="submit" class="w-full bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-700">Submit</button>
      </div>
    </form>
  </div>
</template>

<script setup>
  import { defineProps, reactive, watch } from 'vue'
  import { useMainStore } from '@/stores/main.js'
  import { useRouter } from 'vue-router'

  const props = defineProps({
    project: {
      type: Object,
      required: true,
      default: () => ({
        projectId: '',
        projectName: '',
        projectLocation: '',
        projectStage: 'Planning',
        projectCategory: 'Education',
        projectConstructionStartDate: '',
        projectDetails: '',
        projectCreatorId: '',
        otherCategory: ''
      })
    }
  })

  const router = useRouter()
  const mainStore = useMainStore()

  const project = reactive({ ...props.project })

  watch(() => props.project, (newProject) => {
    Object.assign(project, newProject)
  }, { deep: true })

  const generateProjectId = () => {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'
    let result = ''
    for (let i = 0; i < 6; i++) {
      result += characters.charAt(Math.floor(Math.random() * characters.length))
    }
    return result
  }

  const handleSubmit = () => {
    if (!project.projectId) {
      project.projectId = generateProjectId() // Generate a unique ID if it doesn't exist
    }
    project.projectCreatorId = mainStore.userId 
    if (project.projectCategory != 'Others') {
      project.otherCategory = 'none'
    }
    if (project.projectId.length === 6) {
      if (mainStore.projects.some(p => p.projectId === project.projectId)) {
        alert('Project ID already exists. Please try again.')
      } else {
        if (project.projectId) {
          mainStore.updateProject(project)
        } else {
          mainStore.createProject(project)
        }
        router.push('/dashboard')
      }
    } else {
      alert('Project ID must be 6 characters long.')
    }
  }
</script>
