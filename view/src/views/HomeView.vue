<script setup lang="ts">
import TheWelcome from "../components/TheWelcome.vue";
import { api } from "../utils/backendUtils.ts";

const data = api.get("/api/test").then((response) => {
  console.log(response.data);
}); 
</script>

<template>
  <main></main>
</template>
