export default {
  methods: {
    validateProject(project) {
      if (!project.projectName || project.projectName.length > 200) {
        return 'Project Name is required and must be less than 200 characters.';
      }
      if (!project.projectLocation || project.projectLocation.length > 500) {
        return 'Project Location is required and must be less than 500 characters.';
      }
      if (!project.projectStage) {
        return 'Project Stage is required.';
      }
      if (!project.projectCategory) {
        return 'Project Category is required.';
      }
      if (['Concept', 'Design & Documentation', 'Pre-Construction'].includes(project.projectStage) && new Date(project.projectConstructionStartDate) <= new Date()) {
        return 'Construction Start Date must be in the future for Concept, Design & Documentation, and Pre-Construction stages.';
      }
      if (!project.projectDetails || project.projectDetails.length > 2000) {
        return 'Project Details are required and must be less than 2000 characters.';
      }
      return null;
    }
  }
};
