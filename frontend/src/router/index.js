import { createRouter, createWebHistory } from "vue-router";

import LoginView from "../views/LoginView.vue";
import DashboardView from "../views/DashboardView.vue";

import CoursesView from "@/views/CoursesView.vue";
import JobsView from "@/views/JobsView.vue";
import ProfileView from "@/views/ProfileView.vue";

import CourseDetailsView from "@/views/CourseDetailsView.vue";
import JobDetailsView from "@/views/JobDetailsView.vue";
const routes = [
  {
    path: "/",
    redirect: "/login",
  },
  {
    path: "/login",
    component: LoginView,
  },
  {
    path: "/dashboard",
    component: DashboardView,
  },
  {
    path: "/courses",
    component: CoursesView,
  },
  {
    path: "/jobs",
    component: JobsView,
  },

  {
    path: "/profile",
    component: ProfileView,
  },
  {
    path: "/courses/:id",
    component: CourseDetailsView,
  },
  {
    path: "/jobs/:id",
    component: JobDetailsView,
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
