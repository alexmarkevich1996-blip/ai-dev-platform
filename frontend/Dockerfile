# A local development image running the Vite dev server. The production build is
# static files the backend serves; it needs no image of its own.
FROM node:24-alpine

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .

EXPOSE 5173

CMD ["npm", "run", "dev"]
