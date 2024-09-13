# Usa una imagen base de Nginx
FROM nginx:alpine

# Elimina el contenido predeterminado de Nginx
RUN rm -rf /usr/share/nginx/html

# Copia el contenido del proyecto al directorio predeterminado de Nginx
COPY src /usr/share/nginx/html

# Copia el archivo de configuración principal
COPY config/nginx.conf /etc/nginx/nginx.conf

# Exponemos el puerto 3000 para que coincida con el puerto expuesto en Docker Compose
EXPOSE 3000

# Comando por defecto para ejecutar Nginx en el contenedor
CMD ["nginx", "-g", "daemon off;"]
