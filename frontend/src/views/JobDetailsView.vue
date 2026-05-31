<script setup>
import { onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import api from "../services/api";

const route = useRoute();

const job = ref(null);
const coverLetter = ref("");
const loading = ref(true);
const error = ref("");
const success = ref("");

onMounted(async () => {
    try {
        const response = await api.get(`/jobs/${route.params.id}`);
        job.value = response.data;
    } catch {
        error.value = "Job konnte nicht geladen werden.";
    } finally {
        loading.value = false;
    }
});

const applyToJob = async () => {
    error.value = "";
    success.value = "";

    try {
        await api.post("/applications", {
            jobId: job.value.id,
            coverLetter: coverLetter.value,
        });

        success.value = "Bewerbung wurde erfolgreich gesendet.";
        coverLetter.value = "";
    } catch (err) {
        if (err.response?.data?.message) {
            error.value = err.response.data.message;
        } else {
            error.value = "Bewerbung konnte nicht gesendet werden.";
        }
    }
};
</script>

<template>
    <div class="container mt-4">
        <div v-if="loading" class="alert alert-info">
            Job wird geladen...
        </div>

        <div v-if="error" class="alert alert-danger">
            {{ error }}
        </div>

        <div v-if="success" class="alert alert-success">
            {{ success }}
        </div>

        <div v-if="job" class="card shadow">
            <div class="card-body">
                <h1>{{ job.title }}</h1>

                <p class="text-muted">
                    {{ job.company }} · {{ job.location }}
                </p>

                <span class="badge bg-success mb-3">
                    {{ job.salary }}
                </span>

                <p>{{ job.description }}</p>

                <hr />

                <h4>Bewerben</h4>

                <textarea v-model="coverLetter" class="form-control mb-3" rows="5"
                    placeholder="Schreibe dein Anschreiben..."></textarea>

                <button class="btn btn-primary" @click="applyToJob">
                    Bewerbung senden
                </button>
            </div>
        </div>
    </div>
</template>