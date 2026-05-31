<script setup>
import { ref } from "vue";
import api from "../services/api";

const cvText = ref("");
const result = ref(null);
const loading = ref(false);
const error = ref("");

const analyzeCv = async () => {
    error.value = "";
    result.value = null;
    loading.value = true;

    try {
        const response = await api.post("/ai/analyze-cv", {
            cvText: cvText.value,
        });

        result.value = response.data;
    } catch {
        error.value = "CV konnte nicht analysiert werden.";
    } finally {
        loading.value = false;
    }
};
</script>

<template>
    <div class="container mt-4">
        <h1 class="mb-4">AI CV Analyzer</h1>

        <div v-if="error" class="alert alert-danger">
            {{ error }}
        </div>

        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <label class="form-label">Lebenslauf-Text</label>

                <textarea v-model="cvText" class="form-control mb-3" rows="8"
                    placeholder="Füge hier deinen Lebenslauf-Text ein..."></textarea>

                <button class="btn btn-primary" @click="analyzeCv" :disabled="loading || !cvText">
                    {{ loading ? "Analysiere..." : "CV analysieren" }}
                </button>
            </div>
        </div>

        <div v-if="result" class="card shadow-sm">
            <div class="card-body">
                <h4>Analyse Ergebnis</h4>

                <p class="display-6 text-primary">
                    Score: {{ result.score }}/100
                </p>

                <h5>Gefundene Skills</h5>
                <ul class="list-group mb-3">
                    <li v-for="skill in result.skills" :key="skill" class="list-group-item">
                        ✅ {{ skill }}
                    </li>
                </ul>

                <h5>Empfehlungen</h5>
                <ul class="list-group">
                    <li v-for="suggestion in result.suggestions" :key="suggestion" class="list-group-item">
                        💡 {{ suggestion }}
                    </li>
                </ul>
            </div>
        </div>
    </div>
</template>