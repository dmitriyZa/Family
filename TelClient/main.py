import logging
import openai
import telebot
import requests
from telebot import types
from dotenv import load_dotenv
import os

# Включите логирование
logging.basicConfig(level=logging.INFO)

# Загружаем переменные окружения из .env файла
load_dotenv()

# Получаем токены
API_TELEGRAM_TOKEN = os.getenv('API_TELEGRAM_TOKEN')

bot = telebot.TeleBot(API_TELEGRAM_TOKEN)

# Функция для создания клавиатуры
def create_keyboard():
    markup = types.ReplyKeyboardMarkup(resize_keyboard=True)
    markup.row(types.KeyboardButton("Добавить члена семьи👨‍👩‍👧‍👦"))
    markup.row(types.KeyboardButton("Информация о члене семьиℹ️"))
    markup.row(types.KeyboardButton("Отношения в семье💬"))
    return markup

@bot.message_handler(commands=['start'])
def process_start_command(message: telebot.types.Message):
    bot.send_message(message.chat.id, "Добро пожаловать! Выберите действие с клавиатуры:", reply_markup=create_keyboard())

# Обработчик команд
@bot.message_handler(func=lambda message: True)
def handle_message(message: telebot.types.Message):
    user_input = message.text
    
    if user_input == "Добавить члена семьи👨‍👩‍👧‍👦":
        bot.send_message(message.chat.id, "Введите данные о новом члене семьи в формате 'Имя, Фамилия, Дата рождения':")
        bot.register_next_step_handler(message, add_family_member)
        return
    
    if user_input == "Информация о члене семьиℹ️":
        bot.send_message(message.chat.id, "Введите имя члена семьи для получения информации:")
        bot.register_next_step_handler(message, get_family_info)
        return
    
    if user_input == "Отношения в семье💬":
        bot.send_message(message.chat.id, "Введите имя члена семьи для анализа отношений:")
        bot.register_next_step_handler(message, analyze_relationships)
        return

def add_family_member(message: telebot.types.Message):
    data = message.text.split(',')
    if len(data) == 3:
        family_member = {
            "FirstName": data[0].strip(),
            "LastName": data[1].strip(),
            "DateOfBirth": data[2].strip()
        }
        try:
            response = requests.post("http://localhost:5000/api/family", json=family_member)
            if response.status_code == 201:
                bot.send_message(message.chat.id, "Член семьи добавлен успешно!")
            else:
                bot.send_message(message.chat.id, "Ошибка при добавлении члена семьи.")
        except Exception as e:
            logging.error(f"Ошибка при обращении к API: {e}")
            bot.send_message(message.chat.id, "Ошибка соединения с сервером.")
    else:
        bot.send_message(message.chat.id, "Неверный формат данных. Попробуйте снова.")

def get_family_info(message: telebot.types.Message):
    name = message.text.strip()
    try:
        response = requests.get(f"http://localhost:5000/api/family?name={name}")
        if response.status_code == 200:
            family_info = response.json()
            bot.send_message(message.chat.id, f"Информация о члене семьи: {family_info}")
        else:
            bot.send_message(message.chat.id, "Член семьи не найден.")
    except Exception as e:
        logging.error(f"Ошибка при обращении к API: {e}")
        bot.send_message(message.chat.id, "Ошибка соединения с сервером.")

def analyze_relationships(message: telebot.types.Message):
    name = message.text.strip()
    try:
        response = requests.get(f"http://localhost:5000/api/relationships?name={name}")
        if response.status_code == 200:
            relationships = response.json()
            bot.send_message(message.chat.id, f"Отношения: {relationships}")
        else:
            bot.send_message(message.chat.id, "Отношения для указанного члена семьи не найдены.")
    except Exception as e:
        logging.error(f"Ошибка при обращении к API: {e}")
        bot.send_message(message.chat.id, "Ошибка соединения с сервером.")

# Запуск бота
if __name__ == '__main__':
    bot.polling(none_stop=True)
