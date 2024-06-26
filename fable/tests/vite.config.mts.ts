import { defineConfig } from "vite";

const proxyPort = process.env.SERVER_PROXY_PORT || "5050";
const proxyTarget = "http://localhost:" + proxyPort;

// https://vitejs.dev/config/
export default defineConfig({
    server: {
        port: 8081
    }
});