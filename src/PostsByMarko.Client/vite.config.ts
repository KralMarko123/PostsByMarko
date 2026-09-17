import { fileURLToPath, URL } from "node:url";
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@api": fileURLToPath(new URL("./src/api", import.meta.url)),
      "@auth": fileURLToPath(new URL("./src/auth", import.meta.url)),
      "@components": fileURLToPath(new URL("./src/components", import.meta.url)),
      "@hooks": fileURLToPath(new URL("./src/custom", import.meta.url)),
      "@context": fileURLToPath(new URL("./src/context", import.meta.url)),
      "@typeConfigs": fileURLToPath(new URL("./src/types", import.meta.url)),
      "@assets": fileURLToPath(new URL("./src/assets", import.meta.url)),
      "types": fileURLToPath(new URL("./src/types", import.meta.url)),
      "constants": fileURLToPath(new URL("./src/constants", import.meta.url)),
    },
  },
  server: {
    port: 3000,
    strictPort: true,
  },
  test: {
    environment: "jsdom",
    globals: true,
    restoreMocks: true,
  },
});
