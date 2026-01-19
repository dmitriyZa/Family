# Используем официальный образ Python
FROM python:3.9-slim

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем файлы проекта в контейнер
COPY . .

# Устанавливаем зависимости
RUN pip install --no-cache-dir -r requirements.txt

# Устанавливаем переменные окружения
# Эти переменные могут быть загружены из .env файла
ENV API_TELEGRAM_TOKEN=${API_TELEGRAM_TOKEN}
ENV API_VSEGPT_TOKEN=${API_VSEGPT_TOKEN}

# Команда для запуска бота
CMD ["python", "main.py"]
