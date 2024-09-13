# Usa una imagen base de Nginx
FROM nginx:alpine

COPY config/nginx.conf /etc/nginx/nginx.conf

COPY ./*.html /usr/share/nginx/html/
COPY ./*.css /usr/share/nginx/html/
COPY ./*.png /usr/share/nginx/html/
COPY ./*.js /usr/share/nginx/html/

EXPOSE 3000

CMD ["nginx", "-g", "daemon off;"]
