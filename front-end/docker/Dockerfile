# Usa una imagen base de Nginx
FROM nginx:alpine

# Copia el contenido del proyecto al directorio predeterminado de Nginx
COPY src /usr/share/nginx/html

# Copia el archivo de configuración principal
COPY config/nginx.conf /etc/nginx/nginx.conf

# Copia el archivo de configuración adicional para el servidor
COPY config/default.conf /etc/nginx/conf.d/default.conf

# Exponemos el puerto 3000 para que coincida con el puerto expuesto en Docker Compose
EXPOSE 3000

# Comando por defecto para ejecutar Nginx en el contenedor
CMD ["nginx", "-g", "daemon off;"]
