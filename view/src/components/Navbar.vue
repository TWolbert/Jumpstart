<template>
    <nav class="bg-gray-800 text-white p-4">
        <div class="container mx-auto flex justify-between items-center">
            <router-link to="/" class="text-xl font-bold">
                MyApp
            </router-link>
            <div class="flex space-x-4">
                <router-link to="/" class="hover:underline">
                    Home
                </router-link>
                <router-link to="/login" class="hover:underline">
                    Login
                </router-link>
                <router-link to="/register" class="hover:underline">
                    Register
                </router-link>
            </div>
        </div>
    </nav>
</template>

<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
    name: 'Navbar'
});
</script>