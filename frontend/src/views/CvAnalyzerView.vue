<script setup>
import { ref } from "vue";
import api from "../services/api";

const cvText = ref("");
const selectedFile = ref(null);

const result = ref(null);
const extractedText = ref("");
const loading = ref(false);
const error = ref("");

const analyzeCv = async () => {
    error.value = "";
    result.value = null;
    extractedText.value = "";
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

const handleFileChange = (event) => {
    selectedFile.value = event.target.files[0];
};

const analyzePdf = async () => {
    error.value = "";
    result.value = null;
    extractedText.value = "";
    loading.value = true;

    try {
        const formData = new FormData();
        formData.append("file", selectedFile.value);

        const response = await api.post("/ai/analyze-cv-pdf", formData, {
            headers: {
                "Content-Type": "multipart/form-data",
            },
        });

        result.value = {
            score: response.data.score,
            skills: response.data.skills,
            suggestions: response.data.suggestions,
        };

        extractedText.value = response.data.extractedText;
    } catch (err) {
        error.value =
            err.response?.data?.message || "PDF konnte nicht analysiert werden.";
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

        <div class="row">
            <div class="col-md-6 mb-4">
                <div class="card shadow-sm h-100">
                    <div class="card-body">
                        <h5>Text analysieren</h5>

                        <label class="form-label">Lebenslauf-Text</label>

                        <textarea v-model="cvText" class="form-control mb-3" rows="8"
                            placeholder="Füge hier deinen Lebenslauf-Text ein..."></textarea>

                        <button class="btn btn-primary" @click="analyzeCv" :disabled="loading || !cvText">
                            {{ loading ? "Analysiere..." : "CV analysieren" }}
                        </button>
                    </div>
                </div>
            </div>

            <div class="col-md-6 mb-4">
                <div class="card shadow-sm h-100">
                    <div class="card-body">
                        <h5>PDF hochladen</h5>

                        <label class="form-label">Lebenslauf als PDF</label>

                        <input type="file" class="form-control mb-3" accept="application/pdf"
                            @change="handleFileChange" />

                        <button class="btn btn-success" @click="analyzePdf" :disabled="loading || !selectedFile">
                            {{ loading ? "Analysiere..." : "PDF analysieren" }}
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <div v-if="result" class="card shadow-sm mb-4">
            <div class="card-body">
                <h4>Analyse Ergebnis</h4>

                <p class="display-6 text-primary">
                    Score: {{ result.score }}/100
                </p>

                <h5>Gefundene Skills</h5>

                <ul class="list-group mb-3">
                    <li v-for="skill in result.skills" :key="skill" class="list-group-item">
                        {{ skill }}
                    </li>
                </ul>

                <h5>Empfehlungen</h5>

                <ul class="list-group">
                    <li v-for="suggestion in result.suggestions" :key="suggestion" class="list-group-item">
                        {{ suggestion }}
                    </li>
                </ul>
            </div>
        </div>

        <div v-if="extractedText" class="card shadow-sm">
            <div class="card-body">
                <h5>Aus PDF gelesener Text</h5>

                <pre class="bg-light p-3 rounded">{{ extractedText }}</pre>
            </div>
        </div>
    </div>
</template>