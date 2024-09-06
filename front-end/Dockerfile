# Usa una imagen base de Nginx
FROM nginx:alpine

# Copia el contenido del proyecto al directorio predeterminado de Nginx
COPY . /usr/share/nginx/html

# Exponemos el puerto 3000 para que coincida con el puerto expuesto en Docker Compose
EXPOSE 3000

# Comando por defecto para ejecutar Nginx en el contenedor
CMD ["nginx", "-g", "daemon off;"]
