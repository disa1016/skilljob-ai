<script setup>
const user = JSON.parse(localStorage.getItem("user"));
</script>

<template>
    <div class="container mt-5">

        <div class="card shadow">
            <div class="card-body">

                <h1>
                    Willkommen {{ user?.fullName }}
                </h1>

                <p class="text-muted">
                    Rolle: {{ user?.role }}
                </p>

            </div>
        </div>

    </div>
</template>