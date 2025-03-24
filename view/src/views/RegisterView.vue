<script setup lang="ts">
import { ref } from "vue";
import { api, uploadWithFile } from "../utils/backendUtils.ts";

const username = ref("");
const email = ref("");
const password = ref("");
const profilePicture = ref<File | null>(null);
const previewUrl = ref<string | null>(null);

const handleFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement;
  if (target.files && target.files.length > 0) {
    // Revoke previous URL if exists to avoid memory leaks
    if (previewUrl.value) {
      URL.revokeObjectURL(previewUrl.value);
    }
    profilePicture.value = target.files[0];
    previewUrl.value = URL.createObjectURL(profilePicture.value);
  }
};

const handleSubmit = async () => {
  const data = {
    username: username.value,
    email: email.value,
    password: password.value,
    profilePicture: profilePicture.value,
  }

  const response = await uploadWithFile(data, "post", "/api/test", "profilePicture");
  console.log(response);
};

// Function to fill inputs with dummy data
const fillDummyData = () => {
  username.value = "dummyUser";
  email.value = "dummy@example.com";
  password.value = "password123";
};
</script>

<template>
  <div class="min-h-screen flex flex-col items-center bg-gray-100 dark:bg-gray-900 dark:text-white">
  <div class="w-full max-w-md mt-10">
    <div class="bg-white dark:bg-gray-800 shadow-md rounded px-8 py-6">
    <h1 class="text-2xl font-bold mb-6 text-center">Register</h1>
    <form @submit.prevent="handleSubmit">
      <!-- Username -->
      <div class="mb-4">
      <label for="username" class="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2">
        Username:
      </label>
      <input
        id="username"
        v-model="username"
        type="text"
        required
        class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-100 leading-tight focus:outline-none focus:shadow-outline"
      />
      </div>
      <!-- Email -->
      <div class="mb-4">
      <label for="email" class="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2">
        Email:
      </label>
      <input
        id="email"
        v-model="email"
        type="email"
        required
        class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-100 leading-tight focus:outline-none focus:shadow-outline"
      />
      </div>
      <!-- Password -->
      <div class="mb-4">
      <label for="password" class="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2">
        Password:
      </label>
      <input
        id="password"
        v-model="password"
        type="password"
        required
        class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-100 mb-3 leading-tight focus:outline-none focus:shadow-outline"
      />
      </div>
      <!-- Profile Picture -->
      <div class="mb-6">
      <label for="profilePicture" class="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2">
        Profile Picture:
      </label>
      <!-- Custom file upload button -->
      <label class="bg-gray-700 hover:bg-gray-300 text-white dark:text-white text-sm font-bold py-2 px-4 rounded cursor-pointer inline-block">
        Choose a file
        <input
        id="profilePicture"
        type="file"
        accept="image/*"
        class="hidden"
        @change="handleFileChange"
        />
      </label>
      </div>

      <div class="flex items-center justify-between space-x-4">
      <button
        type="submit"
        class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
      >
        Register
      </button>
      <button
        type="button"
        @click="fillDummyData"
        class="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
      >
        Fill Dummy Data
      </button>
      </div>
    </form>
    <!-- Image preview -->
    <div v-if="previewUrl" class="mt-4 text-center">
      <p class="text-gray-700 dark:text-gray-300 mb-2">Image Preview:</p>
      <img
      :src="previewUrl"
      alt="Profile Preview"
      class="mx-auto rounded w-32 h-32 object-cover shadow-md"
      />
    </div>
    </div>
  </div>
  </div>
</template>
