# build environment
FROM node:14.10.0-alpine as build
WORKDIR /app
ENV PATH /app/node_modules/.bin:$PATH
COPY package.json ./
COPY package-lock.json ./
RUN npm ci --silent
RUN npm install react-scripts@latest -g --silent
COPY . ./
ENV NODE_ENV production
RUN npm run build

# production environment
FROM rtsp/lighttpd
COPY --from=build /app/build /var/www/html/
