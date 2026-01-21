import requests
import certifi
import telebot

API_TOKEN = '8273248222:AAFvNsMk8ZORnv1Jdbs1r20WgJDPEK3vT9U'
bot = telebot.TeleBot(API_TOKEN)

def create_session_with_certifi():
    session = requests.Session()
    session.verify = certifi.where()
    return session

@bot.message_handler(commands=['start', 'help'])
def send_welcome(message):
    bot.reply_to(message, "Hello! I'm EchoBot!")

@bot.message_handler(func=lambda message: True)
def echo_all(message):
    bot.reply_to(message, message.text)

telebot.apihelper.SESSION = create_session_with_certifi()

bot.infinity_polling()
